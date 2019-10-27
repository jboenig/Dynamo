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

namespace Headway.Dynamo.Metadata
{
    /// <summary>
    /// Implements an Http parameter.
    /// </summary>
    public class HttpParameter : Parameter
    {
    }

    /// <summary>
    /// Implements a query string parameter.
    /// </summary>
    public sealed class UrlQueryStringParameter : HttpParameter
    {
    }

    /// <summary>
    /// Implements a URL path parameter.
    /// </summary>
    public sealed class UrlPathParameter : HttpParameter
    {
    }

    /// <summary>
    /// Implements an HTTP header parameter.
    /// </summary>
    public sealed class HttpHeaderParameter : HttpParameter
    {
    }
}
