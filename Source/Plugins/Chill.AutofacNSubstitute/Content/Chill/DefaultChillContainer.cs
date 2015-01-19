using Chill;
using Chill.AutofacNSubstitute;

// This attribute defines which container will be used by default for this assembly

[assembly: ChillContainer(typeof(AutofacNSubstituteChillContainer))]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("Chill")]
#endif
