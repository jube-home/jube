// Accord Math Library
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

namespace Accord.Math.Optimization
{
    using System;
    using System.Runtime.Serialization;
    using Accord.Compat;
    using System.Security.Permissions;

    /// <summary>
    ///   Line Search Failed Exception.
    /// </summary>
    /// 
    /// <remarks>
    ///   This exception may be thrown by the <see cref="BroydenFletcherGoldfarbShanno">L-BFGS Optimizer</see>
    ///   when the line search routine used by the optimization method fails.
    /// </remarks>
    /// 
    [Serializable]
    public class LineSearchFailedException : Exception
    {
        int info;

        /// <summary>
        ///   Gets the error code information returned by the line search routine.
        /// </summary>
        /// 
        /// <value>The error code information returned by the line search routine.</value>
        /// 
        public int Information { get { return info; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineSearchFailedException"/> class.
        /// </summary>
        /// 
        public LineSearchFailedException()
            : base()
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineSearchFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="info">The error code information of the line search routine.</param>
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public LineSearchFailedException(int info, string message)
            : base(message)
        {
            this.info = info;
        }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineSearchFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// 
        public LineSearchFailedException(string message)
            : base(message)
        { }

        /// <summary>
        ///   Initializes a new instance of the <see cref="LineSearchFailedException"/> class.
        /// </summary>
        /// 
        /// <param name="message">Message providing some additional information.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        /// 
        public LineSearchFailedException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
