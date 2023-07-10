/*
 * MinIO .NET Library for Amazon S3 Compatible Cloud Storage, (C) 2021 MinIO, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System.Xml.Serialization;

namespace MinIo.Helpers.DataModel;

[Serializable]
[XmlRoot(ElementName = "Expiration")]
public class Expiration : Duration
{
    public Expiration()
    {
        ExpiredObjectDeleteMarker = default;
    }

    public Expiration(DateTime date, bool deleteMarker = false) : base(date)
    {
        if (date == default)
        {
            ExpiredObjectDeleteMarker = deleteMarker;
            return;
        }

        ExpiredObjectDeleteMarker = default;
    }

    [XmlIgnore] public bool? ExpiredObjectDeleteMarker { get; set; }
}