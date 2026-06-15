// Cave of Wonders
// Copyright (C) 2023-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace DustInTheWind.CaveOfWonders.Ports.InsAccess;

public class CpiImportExportPool
{
    private readonly ICpiImportExportFactory factory;
    private readonly IDictionary<Guid, CpiImportExportDescriptor> descriptors = new Dictionary<Guid, CpiImportExportDescriptor>();

    public CpiImportExportPool(ICpiImportExportFactory factory)
    {
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public void Add(CpiImportExportDescriptor descriptor)
    {
        if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));
        descriptors.Add(descriptor.Id, descriptor);
    }

    public ICpiImportExport Get(Guid id)
    {
        if (!descriptors.TryGetValue(id, out CpiImportExportDescriptor descriptor))
            throw new ArgumentException($"No CPI import/export found for the given ID: {id}", nameof(id));

        return factory.Create(descriptor.Type);
    }
}