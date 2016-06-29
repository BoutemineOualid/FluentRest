using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace FluentRest.Fake
{
    /// <summary>
    /// A memory based fake message store.  The fake response messages are saved and loaded from a ResponseStore using a sh1 has of the URL as the key.
    /// </summary>
    public class MemoryMessageStore : FakeMessageStore
    {
        private static readonly Lazy<MemoryMessageStore> _current = new Lazy<MemoryMessageStore>(() => new MemoryMessageStore());

        /// <summary>
        /// Gets the current singleton instance of <see cref="MemoryMessageStore"/>.
        /// </summary>
        /// <value>The current singleton instance <see cref="MemoryMessageStore"/>.</value>
        public static MemoryMessageStore Current => _current.Value;

#if PORTABLE
        private readonly object _storeLock = new object();
        private readonly System.Collections.Generic.Dictionary<string, FakeResponseContainer> _responseStore = new System.Collections.Generic.Dictionary<string, FakeResponseContainer>();
#else
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, FakeResponseContainer> _responseStore = new System.Collections.Concurrent.ConcurrentDictionary<string, FakeResponseContainer>();
#endif

        /// <summary>
        /// Gets the response store.
        /// </summary>
        /// <value>
        /// The response store.
        /// </value>
        public System.Collections.Generic.IDictionary<string, FakeResponseContainer> ResponseStore => _responseStore;

        /// <summary>
        /// Saves the specified HTTP <paramref name="response" /> to the message store as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message that was sent.</param>
        /// <param name="response">The HTTP response messsage to save.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public override async Task SaveAsync(HttpRequestMessage request, HttpResponseMessage response)
        {
            // don't save content if not success
            if (!response.IsSuccessStatusCode || response.Content == null || response.StatusCode == HttpStatusCode.NoContent)
                return;

            var httpContent = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            var fakeResponse = Convert(response);

            var key = GenerateKey(request);
            var container = new FakeResponseContainer
            {
                HttpContent = httpContent,
                ResponseMessage = fakeResponse
            };


            // save to store
#if PORTABLE
            lock(_storeLock)
                _responseStore[key] = container;
#else
            _responseStore.AddOrUpdate(key, container, (k, o) => container);
#endif
        }

        /// <summary>
        /// Loads an HTTP fake response message for the specified HTTP <paramref name="request" /> as an asynchronous operation.
        /// </summary>
        /// <param name="request">The HTTP request message to load response for.</param>
        /// <returns>
        /// The task object representing the asynchronous operation.
        /// </returns>
        public override Task<HttpResponseMessage> LoadAsync(HttpRequestMessage request)
        {
            var taskSource = new TaskCompletionSource<HttpResponseMessage>();
            var key = GenerateKey(request);

            FakeResponseContainer container;
            bool found = false;

#if PORTABLE
            lock(_storeLock)
                found = _responseStore.TryGetValue(key, out container);
#else
            found = _responseStore.TryGetValue(key, out container);
#endif

            if (!found)
            {
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                httpResponseMessage.RequestMessage = request;
                httpResponseMessage.ReasonPhrase = $"Response for key '{key}' not found";

                taskSource.SetResult(httpResponseMessage);
                return taskSource.Task;
            }

            var fakeResponse = container.ResponseMessage;
            var httpResponse = Convert(fakeResponse);

            taskSource.SetResult(httpResponse);

            if (container.HttpContent == null)
                return taskSource.Task;

            var httpContent = new ByteArrayContent(container.HttpContent);

            // copy headers
            foreach (var header in fakeResponse.ResponseHeaders)
                httpContent.Headers.TryAddWithoutValidation(header.Key, header.Value);

            httpResponse.Content = httpContent;

            return taskSource.Task;
        }


        /// <summary>
        /// Register a fake response using the specified fluent <paramref name="builder"/> action.
        /// </summary>
        /// <param name="builder">The fluent container builder.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Register(Action<FakeContainerBuilder> builder)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));

            var container = new FakeResponseContainer();
            var containerBuilder = new FakeContainerBuilder(container);

            builder(containerBuilder);

            // save to store
            var key = container.RequestUri.ToString();

#if PORTABLE
            lock(_storeLock)
                _responseStore[key] = container;
#else
            _responseStore.AddOrUpdate(key, container, (k, o) => container);
#endif
        }
    }
}