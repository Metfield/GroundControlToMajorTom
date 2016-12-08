using UnityEngine.Networking;

namespace Shared
{
    public class CargoLaunchMsg : MessageBase
    {
        public float successRatio;
        public int[] cargo;
    }
}
