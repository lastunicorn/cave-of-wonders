using DustInTheWind.CaveOfWonders.DataTypes;
using DustInTheWind.CaveOfWonders.Domain;
using FluentAssertions;

namespace CaveOfWonders.Tests.GemTests;

public class EqualsTests
{
	[Fact]
	public void HavingSameParametersInDifferentOrder_WhenGemsAreCompared_ThenGemsAreEqual()
	{
		Gem gem1 = new()
		{
			Date = new DateTime(2023, 12, 15),
			Category = GemCategory.Gain,
			Amount = 22.06m,
			Description = "Interest revenue 11/23",
			Parameters =
			{
				new GemParameter { Key = "Counterpart", Value = "Quanloop" },
				new GemParameter { Key = "Account", Value = "" },
				new GemParameter { Key = "BicSwift", Value = "" },
				new GemParameter { Key = "Balance", Value = "1753.36" }
			}
		};

		Gem gem2 = new()
		{
			Date = new DateTime(2023, 12, 15),
			Category = GemCategory.Gain,
			Amount = 22.06m,
			Description = "Interest revenue 11/23",
			Parameters =
			{
				new GemParameter { Key = "Account", Value = "" },
				new GemParameter { Key = "Balance", Value = "1753.36" },
				new GemParameter { Key = "BicSwift", Value = "" },
				new GemParameter { Key = "Counterpart", Value = "Quanloop" }
			}
		};

		bool areEqual = gem1.Equals(gem2);

		areEqual.Should().BeTrue();
	}

	[Fact]
	public void HavingDifferentParameterValues_WhenGemsAreCompared_ThenGemsAreNotEqual()
	{
		Gem gem1 = new()
		{
			Date = new DateTime(2023, 12, 15),
			Category = GemCategory.Gain,
			Amount = 22.06m,
			Description = "Interest revenue 11/23",
			Parameters =
			{
				new GemParameter { Key = "Balance", Value = "1753.36" }
			}
		};

		Gem gem2 = new()
		{
			Date = new DateTime(2023, 12, 15),
			Category = GemCategory.Gain,
			Amount = 22.06m,
			Description = "Interest revenue 11/23",
			Parameters =
			{
				new GemParameter { Key = "Balance", Value = "1731.30" }
			}
		};

		bool areEqual = gem1.Equals(gem2);

		areEqual.Should().BeFalse();
	}
}
