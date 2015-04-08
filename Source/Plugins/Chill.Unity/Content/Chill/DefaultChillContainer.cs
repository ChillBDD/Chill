using Chill;
using Chill.Unity;

// This attribute defines which container will be used by default for this assembly

[assembly: ChillContainer(typeof(UnityChillContainer))]

#if SILVERLIGHT
[assembly: InternalsVisibleTo("Chill")]
#endif
