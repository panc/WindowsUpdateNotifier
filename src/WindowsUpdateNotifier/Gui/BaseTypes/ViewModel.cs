using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsUpdateNotifier
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property changed event to notify that a property on this object has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        public void RaisePropertyChanged(string propertyName = null)
        {
            if (propertyName == null)
                return;

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// Raises the property changed event to notify that a property on this object has changed.
        /// </summary>
        /// <param name="propertyExpression">A lambda expression specifying the property that has changed.</param>
        public void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            RaisePropertyChanged(GetPropertyName(propertyExpression));
        }

        protected string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException("Not a 'MemberAccessExpression'");

            var propertyInfo = memberExpression.Member as PropertyInfo;
            if (propertyInfo == null)
                throw new ArgumentException("Not a 'PropertyExpression'");

            if (propertyInfo.GetGetMethod(true).IsStatic)
                throw new ArgumentException("Not a 'MemberInstanceExpression'");

            return memberExpression.Member.Name;
        }
    }
}