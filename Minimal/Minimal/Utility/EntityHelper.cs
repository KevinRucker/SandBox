// Author: Kevin Rucker
// License: BSD 3-Clause
// Copyright (c) 2014 - 2015, Kevin Rucker
// All rights reserved.

// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// 3. Neither the name of the copyright holder nor the names of its contributors
//    may be used to endorse or promote products derived from this software without
//    specific prior written permission.
//
// Disclaimer:
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY
// THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using Minimal.Framework.BaseClasses;
using System;
using System.Reflection;
using System.Threading;

namespace Minimal.Utility
{
    /// <summary>
    /// Entity helper class
    /// </summary>
    public static class EntityHelper
    {
        private static readonly object _lock = new object();

        /// <summary>
        /// Set entity base class' Id field (entities inheriting PersistentEntityBase)
        /// </summary>
        /// <typeparam name="TEntity">The entity's type</typeparam>
        /// <param name="entity">The entity</param>
        /// <param name="Id">The value for Id</param>
        public static void SetEntityId<TEntity>(TEntity entity, int Id)
            where TEntity : PersistentEntityBase
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var baseType = entity.GetType().BaseType;
                var info = baseType.GetField("_entityId", BindingFlags.NonPublic | BindingFlags.Instance);
                info.SetValue(entity, Id);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Invoke entity base class' AcceptChanges method (entities inheriting PersistentEntityBase)
        /// </summary>
        /// <typeparam name="TEntity">The entity's type</typeparam>
        /// <param name="entity">The entity</param>
        public static void EntityAcceptChanges<TEntity>(TEntity entity)
            where TEntity : PersistentEntityBase
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var info = entity.GetType().GetMethod("AcceptChanges", BindingFlags.NonPublic | BindingFlags.Instance);
                info.Invoke(entity, null);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }

        /// <summary>
        /// Set entity class' ModifiedDate field (entities inheriting PersistentEntityBase)
        /// </summary>
        /// <typeparam name="TEntity">The entity's type</typeparam>
        /// <param name="entity">The entity</param>
        /// <param name="date">The value for ModifiedDate</param>
        public static void EntityModifiedDate<TEntity>(TEntity entity, DateTime date)
            where TEntity : PersistentEntityBase
        {
            var lockTaken = false;
            try
            {
                Monitor.Enter(_lock, ref lockTaken);
                var type = entity.GetType();
                var info = type.GetField("_modifiedDate", BindingFlags.NonPublic | BindingFlags.Instance);
                info.SetValue(entity, date);
            }
            finally
            {
                if (lockTaken) { Monitor.Exit(_lock); }
            }
        }
    }
}
