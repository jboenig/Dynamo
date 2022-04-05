////////////////////////////////////////////////////////////////////////////////
// Copyright 2019 Jeff Boenig
//
// This file is part of Headway.Dynamo.Restful.
//
// Headway.Dynamo.Restful is free software: you can redistribute it and/or
// modify it under the terms of the GNU General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Headway.Dynamo.Restful is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR PARTICULAR PURPOSE. See the GNU General
// Public License for more details.
//
// You should have received a copy of the GNU General Public License along with
// Headway.Dynamo.Restful. If not, see http://www.gnu.org/licenses/.
////////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json.Linq;

namespace Headway.Dynamo.Restful.UnitTests.Mockdata
{
    internal sealed class JsonPlaceholderContext
    {
        public int Id { get; set; }
        public JObject ResponseContent { get; set; }
        public object RequestContent { get; set; }
    }
}
