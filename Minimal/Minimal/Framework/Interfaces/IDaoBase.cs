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

using System.Collections.Generic;

namespace Minimal.Framework.Interfaces
{
    /// <summary>
    /// Base interface for Data Access Objects (DAO)
    /// </summary>
    /// <typeparam name="TEntity">Type of entity used by DAO</typeparam>
    interface IDaoBase<TEntity>
    {
        /// <summary>
        /// Fetch entity by Id
        /// </summary>
        /// <param name="ID">The Id</param>
        /// <returns>TEntity instance</returns>
        TEntity Fetch(int ID);
        /// <summary>
        /// Fetch filtered List of TEntity
        /// </summary>
        /// <param name="filter">Filter used to obtain list</param>
        /// <returns>List of TEntity instances</returns>
        IList<TEntity> FetchList(object filter);
        /// <summary>
        /// Fetch unfiltered List of TEntity
        /// </summary>
        /// <returns>List of TEntity instances</returns>
        IList<TEntity> FetchAll();
        /// <summary>
        /// Save (Insert/Update) TEntity
        /// </summary>
        /// <param name="entity">The entity to save</param>
        /// <returns>TEntity instance</returns>
        TEntity Save(TEntity entity);
        /// <summary>
        /// Delete TEntity
        /// </summary>
        /// <param name="entity">The entity to delete</param>
        void Delete(TEntity entity);
    }
}
