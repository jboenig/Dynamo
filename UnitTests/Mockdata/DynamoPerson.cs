using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Threading.Tasks;
using Headway.Dynamo.Runtime;
using Headway.Dynamo.Metadata;

namespace Headway.Dynamo.UnitTests.Mockdata
{
    public class DynamoPerson : Runtime.Dynamo
    {
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
