﻿// Accord Core Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord
{
    using System;
    using System.Runtime.Serialization;
    using Accord.Compat;

    /// <summary>
    ///   Non-Symmetric Matrix Exception.
    /// </summary>
    /// 
    /// <remarks><para>The not symmetric matrix exception is thrown in cases where a method 
    /// expects a matrix to be symmetric but it is not.</para>
    /// </remarks>
    /// 
    [Serializable]
    public class NonSymmetricMatrixException : InvalidOperationException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
        /// </summary>
        public NonSymmetricMatrixException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public NonSymmetricMatrixException(string message) :
            base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="NonSymmetricMatrixException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public NonSymmetricMatrixException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
