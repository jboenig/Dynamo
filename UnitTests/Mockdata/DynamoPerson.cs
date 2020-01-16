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
using Newtonsoft.Json;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.UnitTests.Mockdata
{
    [JsonObject]
    public class DynamoPerson : DynamoObject
    {
        public DynamoPerson()
        {
        }
        public DynamoPerson(ObjectType objType) :
            base(objType)
        {
        }

        /// <summary>
        /// Gets or sets the unique ID of this person.
        /// </summary>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the first name of this person.
        /// </summary>
        public string FirstName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the middle name of this person.
        /// </summary>
        public string MiddleName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last name of this person.
        /// </summary>
        public string LastName
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the name of this <see cref="Person"/> in the specified
        /// format.
        /// </summary>
        /// <param name="nameFormat">
        /// Format to generate and return
        /// </param>
        /// <returns>
        /// Returns the name of this <see cref="Person"/> in the
        /// specified <see cref="NameFormats"/>.
        /// </returns>
        public string GetFormattedName(NameFormats nameFormat)
        {
            if (nameFormat == NameFormats.FirstMiddleInitialLast)
            {
                if (!string.IsNullOrEmpty(this.MiddleName))
                {
                    return string.Format("{0} {1}. {2}", this.FirstName, this.MiddleName.Substring(0, 1).ToUpper(), this.LastName);
                }
                else
                {
                    return string.Format("{0} {1}", this.FirstName, this.LastName);
                }
            }
            else if (nameFormat == NameFormats.LastFirstMiddleInitial)
            {
                if (!string.IsNullOrEmpty(this.MiddleName))
                {
                    return string.Format("{0}, {1} {2}.", this.LastName, this.FirstName, this.MiddleName.Substring(0, 1).ToUpper());
                }
                else
                {
                    return string.Format("{0}, {1}", this.LastName, this.FirstName);
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets or sets the date of birth for this <see cref="Person"/>.
        /// </summary>
        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public Address Addr
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the age of the <see cref="Person"/> in years.
        /// </summary>
        public int AgeInYears
        {
            get
            {
                var curDateTime = DateTime.Now;
                var dob = this.DateOfBirth;

                return (curDateTime.Year - dob.Year - 1) +
                        (((curDateTime.Month > dob.Month) ||
                        ((curDateTime.Month == dob.Month) && (curDateTime.Day >= dob.Day))) ? 1 : 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public object ExtraStuff
        {
            get;
            set;
        }

        /// <summary>
        /// Returns human-readable string for this object.
        /// </summary>
        /// <returns>
        /// Returns the name of this <see cref="Person"/> in
        /// First name MI Last name format.
        /// </returns>
        public override string ToString()
        {
            return this.GetFormattedName(NameFormats.FirstMiddleInitialLast);
        }
    }
}
