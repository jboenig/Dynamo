////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.
//
// Headway.Dynamo is free software: you can redistribute it and/or modify it under
// the terms of the GNU General Public License as published by the Free Software
// Foundation, either version 3 of the License, or (at your option) any later
// version.
//
// Headway.Dynamo is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR PARTICULAR PURPOSE. See the GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;

namespace Headway.Dynamo.Runtime
{
    internal class DynamoMetaObject : DynamicMetaObject
    {
        public DynamoMetaObject(Expression parameter,
                Dynamo dynamo) :
            base(parameter, BindingRestrictions.Empty, dynamo)
        {
        }

        public override DynamicMetaObject BindSetMember(SetMemberBinder binder,
             DynamicMetaObject value)
        {
            // Method to call in the containing class:
            string methodName = "SetPropertyValue";

            // setup the binding restrictions.
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            // setup the parameters:
            Expression[] args = new Expression[2];
            // First parameter is the name of the property to Set
            args[0] = Expression.Constant(binder.Name);
            // Second parameter is the value
            args[1] = Expression.Convert(value.Expression, value.RuntimeType);

            // Setup the 'this' reference
            Expression self = Expression.Convert(Expression, LimitType);

            // Setup the method call expression
            var setPropertyValueMethod = typeof(Dynamo).GetMethod(methodName);
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
            // Method call in the containing class:
            string methodName = "GetPropertyValue";

            // setup the binding restrictions.
            BindingRestrictions restrictions =
                BindingRestrictions.GetTypeRestriction(Expression, LimitType);

            // setup the parameters:
            Expression[] args = new Expression[1];
            // First parameter is the name of the property to Set
            args[0] = Expression.Constant(binder.Name);

            // Setup the 'this' reference
            Expression self = Expression.Convert(Expression, LimitType);

            var getPropertyValueMethod = typeof(Dynamo).GetMethod(methodName);
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
            typeof(Dynamo).GetMethod("WriteMethodInfo"),
            parameters),
            BindingRestrictions.GetTypeRestriction(Expression, LimitType));
            return methodInfo;
        }    }
}