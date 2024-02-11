// Accord Core Library
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

    /// <summary>
    ///   Runtime cast.
    /// </summary>
    /// 
    /// <typeparam name="T">The target type.</typeparam>
    /// <typeparam name="U">The source type.</typeparam>
    /// 
    internal struct Cast<T, U>
    {
        private T value;

        /// <summary>
        ///   Gets the value being casted.
        /// </summary>
        /// 
        public T Value { get { return value; } }

        /// <summary>
        ///   Initializes a new instance of the <see cref="Cast{T,U}"/> struct.
        /// </summary>
        /// 
        public Cast(U value)
        {
            this.value = (T)System.Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Performs an implicit conversion from <typeparamref name="U"/> to <see cref="Cast{T,U}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T, U>(U value)
        {
            return new Cast<T, U>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Cast{T,U}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(Cast<T, U> value)
        {
            return value.Value;
        }
    }

    /// <summary>
    ///   Runtime cast.
    /// </summary>
    /// 
    /// <typeparam name="T">The target type.</typeparam>
    /// 
    internal struct Cast<T>
    {
        private T value;

        /// <summary>
        ///   Gets the value being casted.
        /// </summary>
        /// 
        public T Value { get { return value; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="cast{T}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public Cast(object value)
        {
            this.value = (T)System.Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Double"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(double value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.Single"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(float value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Decimal"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(Decimal value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Byte"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(Byte value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="SByte"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(SByte value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int16"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(Int16 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt16"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(UInt16 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int32"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(Int32 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt32"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(UInt32 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Int64"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(Int64 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UInt64"/> to <see cref="cast{T}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator Cast<T>(UInt64 value)
        {
            return new Cast<T>(value);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="cast{T}"/> to <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator T(Cast<T> value)
        {
            return value.Value;
        }
    }
}
