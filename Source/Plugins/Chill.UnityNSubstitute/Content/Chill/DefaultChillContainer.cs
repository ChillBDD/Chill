using Chill;
using Chill.UnityNSubstitute;

// This attribute defines which container will be used by default for this assembly

[assembly: ChillContainer(typeof(UnityNSubstituteChillContainer))]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("Chill")]
#endif
