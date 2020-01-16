////////////////////////////////////////////////////////////////////////////////
// The MIT License(MIT)
// Copyright(c) 2020 Jeff Boenig
// This file is part of Headway.Dynamo
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;

namespace Headway.Dynamo.Runtime
{
    internal class DynamoMetaObject : DynamicMetaObject
    {
        private const string BindSetMemberMethodName = "SetPropertyValue";
        private const string BindGetMemberMethodName = "GetPropertyValue";

        public DynamoMetaObject(Expression parameter,
                DynamoObject dynamo) :
            base(parameter, BindingRestrictions.Empty, dynamo)
        {
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder,
             DynamicMetaObject value)
        {
            // setup the binding restrictions.
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            // Setup arguments
            Expression[] args = new Expression[2]
            {
                Expression.Constant(binder.Name),
                Expression.Convert(value.Expression, value.RuntimeType)
            };

            // Setup the 'this' reference
            Expression self = Expression.Convert(Expression, LimitType);

            // Setup the method call expression
            var setPropertyValueMethod = typeof(DynamoObject).GetMethod(BindSetMemberMethodName);
            var setPropertyValueGenericMethod = setPropertyValueMethod.MakeGenericMethod(new Type[] { value.RuntimeType });

            Expression setPropertyValueExpr = Expression.Call(self,
                setPropertyValueGenericMethod,
                args);

            // Create a meta object to invoke Set later:
            DynamicMetaObject setPropertyValueMetaObj = new DynamicMetaObject(
                setPropertyValueExpr,
                restrictions);

            // return that dynamic object
            return setPropertyValueMetaObj;
        }

        public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
        {
            // setup the binding restrictions.
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            // Setup arguments
            Expression[] args = new Expression[1]
            {
                Expression.Constant(binder.Name)
            };

            // Setup the 'this' reference
            Expression self = Expression.Convert(Expression, LimitType);

            // Use reflection to get generic getter method
            var getPropertyValueMethod = typeof(DynamoObject).GetMethod(BindGetMemberMethodName);
            var getPropertyValueGenericMethod = getPropertyValueMethod.MakeGenericMethod(new Type[] { typeof(object) });

            Expression getPropertyValueExpr = Expression.Call(self,
                getPropertyValueGenericMethod,
                args);

            // Create a meta object to invoke Set later:
            DynamicMetaObject getPropertyValueMetaObj = new DynamicMetaObject(
                getPropertyValueExpr,
                restrictions);

            return getPropertyValueMetaObj;
        }

        public override DynamicMetaObject BindInvokeMember(
         InvokeMemberBinder binder, DynamicMetaObject[] args)
        {
            StringBuilder paramInfo = new StringBuilder();
            paramInfo.AppendFormat("Calling {0}(", binder.Name);
            foreach (var item in args)
                paramInfo.AppendFormat("{0}, ", item.Value);
            paramInfo.Append(")");

            Expression[] parameters = new Expression[]
            {
                Expression.Constant(paramInfo.ToString())
            };
            DynamicMetaObject methodInfo = new DynamicMetaObject(
            Expression.Call(
            Expression.Convert(Expression, LimitType),
            typeof(DynamoObject).GetMethod("WriteMethodInfo"),
            parameters),
            BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return methodInfo;
        }
    }
}