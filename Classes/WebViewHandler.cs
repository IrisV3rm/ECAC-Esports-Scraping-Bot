using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

#pragma warning disable CS4014

namespace ECAC_eSports_Scraper.Classes
{
    public class WebViewHandler
    {
        private bool _initialized;
        public static HttpClient PublicClient = new();
        private CoreWebView2 _internalWebView;
        private TaskCompletionSource<bool> _navCompleted;

        public async Task<Task> InitWebView(WebView2 internalView)
        {

            internalView.CoreWebView2InitializationCompleted += delegate
            {
                _internalWebView = internalView.CoreWebView2;
                _internalWebView.NavigationCompleted += delegate
                {
                    Debug.WriteLine("Nav Complated");
                    _navCompleted.TrySetResult(true);
                };

                _initialized = true;
            };

            internalView.EnsureCoreWebView2Async();

            while (!_initialized) await Task.Delay(50);
            
            return Task.CompletedTask;
        }

        public async Task<string> GetPageSource()
        {
            Debug.WriteLine("GOT PAGE SOURCE");
            return await _internalWebView.ExecuteScriptAsync("new XMLSerializer().serializeToString(document);");
        }

        public async Task<string> NavigateAndGetSource(string url)
        {
            _navCompleted = new TaskCompletionSource<bool>();
            _internalWebView.Navigate(url);
            await _navCompleted.Task;
            return await GetPageSource();
        }

        public async void SendTextToElement(string text, string elementId = "")
        {
            await _internalWebView.ExecuteScriptAsync($@"
                function set(obj, callback) {{
	                callback(obj);

	                for (let [k, v] of Object.entries(obj)) {{
		                if (k.includes('reactProps') && v.onChange) {{
			                v.onChange({{target: obj}});
		                }}
	                }}
                }}
                set(document.getElementById('{elementId}'), obj => obj.value='{text}');
            ");
        }

        public async void ClickElementByClassName(string elementId)
        {
            await _internalWebView.ExecuteScriptAsync($"document.getElementsByClassName('{elementId}')[0].click();");
        }

    }
}
