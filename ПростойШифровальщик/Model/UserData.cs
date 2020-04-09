using System;
using System.Collections.Generic;

namespace ПростойШифровальщик.Model
{
    [Serializable]
    public class UserData 
    {
        /// <summary>
        /// Основной текст
        /// </summary>
        public string GeneralText;

        public List<InformationGroup> InformationGroup=new List<InformationGroup>();
    }
}