// MealPlanPage.xaml.cs
using System.Windows;

namespace FormAI
{
    public partial class MealPlanPage : BaseWindow
    {
        // The constructor now calls an async method.
        public MealPlanPage(string windowTitle, string markdownContent)
        {
            InitializeComponent();
            HeaderText.Text = windowTitle;

            // Call the async method to initialize and load the content.
            InitializeBrowserAsync(markdownContent);
        }

        // We use an async method to handle the WebView2 initialization.
        private async void InitializeBrowserAsync(string markdownContent)
        {
            // This is the modern, reliable way to set the content.
            // It ensures the browser component is ready before we try to use it.
            await MarkdownBrowser.EnsureCoreWebView2Async(null);

            string htmlBody = Markdig.Markdown.ToHtml(markdownContent);

            // The same CSS as before, which WebView2 will understand perfectly.
            string styledHtml = $@"
        <html>
            <head>
                <style>
                    body {{ 
                        font-family: Segoe UI, sans-serif; 
                        background-color: #1E1E1E; 
                        color: #E0E0E0;
                        font-size: 16px; /* CHANGE 1: Make all base text a bit bigger */
                        line-height: 1.6;  /* Add a little more space between lines */
                    }}
                    h1, h2, h3 {{ 
                        color: #8B5CF6; 
                        padding-bottom: 5px;
                        border-bottom: none; /* CHANGE 2: Remove the ugly lines */
                        margin-top: 30px; /* Add some space above headers */
                    }}
            strong {{
                color: #a7a7a7;
                font-weight: 600;
            }}
            </style>
                </head>
                <body>
            {htmlBody}
            </body>
                </html>";
            
            // This is the new way to load the HTML string.
            MarkdownBrowser.CoreWebView2.NavigateToString(styledHtml);
        }
    }
}