#region NameSpace
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text; 
#endregion

namespace WBid.WBidiPad.Model
{
    public class ObservableObject : INotifyPropertyChanged
    {

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        public static event PropertyChangedEventHandler PropertyChangedStatic;

        #endregion

        #region Protected members

        protected static void RaisePropertyChangedStatic<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) return;

            var propertyName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(propertyName)) return;
            RaisePropertyChangedStatic(propertyName);
        }

        protected static void RaisePropertyChangedStatic(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChangedStatic;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(null, e);
            }
        }

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) return;

            var propertyName = memberExpression.Member.Name;
            if (string.IsNullOrEmpty(propertyName)) return;
            RaisePropertyChanged(propertyName);
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        #endregion
    }
}
