using UnityEngine;

namespace UI
{
    public class ChestPanelController : MonoBehaviour
    {
        private void CallProcessChestItems()
        {
            ManagerLocator.Instance.Get<GameManager>().ProcessChestItems();
        }

        private void PlaySe()
        {
            GetComponent<AudioSource>().Play();
        }
    }
}