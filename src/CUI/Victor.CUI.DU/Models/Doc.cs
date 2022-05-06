using System;
using System.Collections.Generic;
using System.Linq;


using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;
namespace Victor.CUI.DU.Models
{
    public class Doc
    {
        public Doc(string filename, IEnumerable<FormPage> pages = null)
        {
            FileName = filename;
            Pages = pages?.ToList() ?? new List<FormPage>();
        }

        #region Properties
        public string FileName;
        public List<FormPage> Pages;
        #endregion
    }
}
