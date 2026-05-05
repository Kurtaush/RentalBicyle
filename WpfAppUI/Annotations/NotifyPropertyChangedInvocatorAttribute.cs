using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppLab1_8Карбушев.Annotations
{
    /// <summary>
    /// Указывает, что метод используется для уведомления об изменении свойства.
    /// Помогает ReSharper автоматически генерировать код для INotifyPropertyChanged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class NotifyPropertyChangedInvocatorAttribute : Attribute
    {
        public NotifyPropertyChangedInvocatorAttribute() { }

        public NotifyPropertyChangedInvocatorAttribute(string parameterName)
        {
            ParameterName = parameterName;
        }

        public string ParameterName { get; }
    }
}
