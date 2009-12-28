using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace System.ComponentModel.DataAnnotations
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple=false)]
    public class StringMinimumLengteAttribute : ValidationAttribute
    {
        // Fields
        public int MinimumLenght { get; set;}

        // Methods
        public StringMinimumLengteAttribute(int minimumLength)
            : base(delegate { return Properties.Resources.StringMinimumLengthAttribute_ErrorMessage; })
        {
            if (minimumLength < 0)
            {
                throw new ArgumentOutOfRangeException("minimumLength", minimumLength,Properties.Resources.StringMinimumLengthAttribute_InvalidMinimumLenght);
            }
            this.MinimumLenght = minimumLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, base.ErrorMessageString, new object[] { name, this.MinimumLenght });
        }

        public override bool IsValid(object value)
        {
            if (value != null)
            {
                return (((string) value).Length >= this.MinimumLenght);
            }
            return false;
        }

    }

}