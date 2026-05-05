using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppLab1_8Карбушев.Annotations
{
    /// <summary>
    /// Интерфейс-маркер для обозначения классов, использующих уведомления об изменении свойств.
    /// Может использоваться для статического анализа и инструментов генерации кода.
    /// </summary>
    public interface NotifyPropertyChangedInvocator
    {
        // Этот интерфейс служит маркером и не требует реализации методов.
        // Он помогает инструментам (например, ReSharper) распознавать паттерн INotifyPropertyChanged.
    }
}
