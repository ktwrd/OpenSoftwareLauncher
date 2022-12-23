﻿using OSLCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenSoftwareLauncher.DesktopWinForms
{
    public partial class HttpExceptionModal : Form
    {
        public HttpExceptionModal()
        {
            InitializeComponent();
            Exception = new HttpException();
            Code = 0;
            Content = "";
            URL = "";
            Stack = Environment.StackTrace;
        }
        public HttpExceptionModal(HttpException exception, int code, string content, string url)
        {
            InitializeComponent();
            Exception = exception;
            Code = code;
            Content = content;
            URL = url;
            Stack = Environment.StackTrace;
        }
        public HttpException Exception { get; set; }
        public int Code { get; set; }
        public string Content { get; set; }
        public string URL { get; set; }
        public string Stack { get; set; }
        public void Locale()
        {
            Text = LocaleManager.Get("Title_HttpException");
            var injectDict = new Dictionary<string, object>()
            {
                {"url", URL},
                {"code", Code },
                {"content", Content },
                {"stack", Stack },
                {"message", Exception.Message }
            };
            labelURL.Text = LocaleManager.Get("HttpException_URL", inject: injectDict);
            textBoxStack.Text = Stack;
            labelExceptionMessage.Text = LocaleManager.Get("HttpException_Message", inject: injectDict);
        }
    }
}