namespace DustInTheWind.OperationEngine.Extensions.DependencyInjection;

internal static class TypeExtensions
{
	public static bool ImplementsInterface(this Type type, Type interfaceType)
	{
		if (!interfaceType.IsInterface)
			return false;

		while (type != null && type != typeof(object))
		{
			IEnumerable<Type> implementedInterfaceTypes = type.GetInterfaces();

			foreach (Type implementedInterfaceType in implementedInterfaceTypes)
			{
				Type currentInterfaceType = implementedInterfaceType.IsGenericType
					? implementedInterfaceType.GetGenericTypeDefinition()
					: implementedInterfaceType;

				if (currentInterfaceType == interfaceType)
					return true;
			}

			type = type.BaseType;
		}

		return false;
	}

	public static bool ImplementsAnyInterface(this Type type, params Type[] interfaceTypes)
	{
		Type[] actualInterfaceTypes = interfaceTypes
			.Where(x => x != null && x.IsInterface)
			.ToArray();

		if (actualInterfaceTypes.Length == 0)
			return false;

		while (type != null && type != typeof(object))
		{
			IEnumerable<Type> implementedInterfaceTypes = type.GetInterfaces();

			foreach (Type implementedInterfaceType in implementedInterfaceTypes)
			{
				Type currentInterfaceType = implementedInterfaceType.IsGenericType
					? implementedInterfaceType.GetGenericTypeDefinition()
					: implementedInterfaceType;

				if (actualInterfaceTypes.Contains(currentInterfaceType))
					return true;
			}

			type = type.BaseType;
		}

		return false;
	}
}