using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

using BrowseSharp;
using BrowseSharp.Common;
namespace Victor
{
    public class BrowserSession : Api
    {
        #region Constructors
        public BrowserSession(CancellationToken ct) : base(ct)
        {
            Browser = new Browser();
            Initialized = true;
        }

        public BrowserSession() : this(Ct) { }
        #endregion

        #region Properties
        public Browser Browser { get; }

        public bool FollowRedirects
        {
            get => Browser.FollowRedirects;
            set => Browser.FollowRedirects = value;
        }
        public CookieContainer CookieContainer => Browser?.CookieContainer; 
        #endregion

        #region Methods
        public async Task<T> NavigateAsync<T>(Uri uri, Dictionary<string, string> headers, Dictionary<string, string> formData,
            Func<IDocument, T> action)
        {
            ThrowIfNotInitialized();
            var doc = await Browser.NavigateAsync(uri, headers, formData);
            //doc.
            return action(doc);
         
        }
        #endregion

        #region Fields
        
        #endregion
    }
}
