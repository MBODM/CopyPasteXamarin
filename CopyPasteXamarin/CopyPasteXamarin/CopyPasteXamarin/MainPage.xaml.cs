using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace CopyPasteXamarin
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            this.button.Clicked += Button_Clicked;
            this.editor.TextChanged += Editor_TextChanged;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            var text = await GetTextFromWebApiAsync(this.editor.Text);

            switch (Device.OS)
            {
                case TargetPlatform.iOS:
                    UIPasteboard clipboard = UIPasteboard.General;
                    clipboard.String = text.Trim();
                    break;
                case TargetPlatform.Android:
                case TargetPlatform.Windows:
                case TargetPlatform.WinPhone:
                case TargetPlatform.Other:
                    throw new NotSupportedException("This platform is not supported.");
            }
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            var editor = sender as Editor;

            if (editor != null)
            {
                var text = e.NewTextValue;

                if (!string.IsNullOrEmpty(text))
                {
                    if ((text.Length > 3) || (TextContainsOnlyNumbers(text) == false))
                    {
                        editor.Text = e.OldTextValue;
                    }
                }
            }
        }

        public async Task<string> GetTextFromWebApiAsync(string id)
        {
            using (var client = new HttpClient())
            {
                var url = "http://mbodm.com/CopyPasteApi/api/values/" + id;

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }
            }
        }

        private bool TextContainsOnlyNumbers(string text)
        {
            var result = false;

            if (string.IsNullOrEmpty(text))
            {
                result = false;
            }

            if (text.Length == 1)
            {
                result = CharIsNumber(text[0]);
            }

            if (text.Length == 2)
            {
                result = CharIsNumber(text[0]) && CharIsNumber(text[1]);
            }

            if (text.Length == 3)
            {
                result = CharIsNumber(text[0]) && CharIsNumber(text[1]) && CharIsNumber(text[2]);
            }

            return result;
        }

        private bool CharIsNumber(char c)
        {
            var numbers = new List<char>() { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };

            return numbers.Contains(c);
        }
    }
}
