using Cysharp.Threading.Tasks;
using FIO.ModularAddressableSystem;
using Sirenix.OdinInspector;
using System.Threading.Tasks;
using TMS.Feedback;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class TestScript : MonoBehaviour
{
    [Inject] private AddressablesManager addressablesManager;

    //[Inject] private IFeedbackManager feedbackManager;

    //[SerializeField] private AudioType audioType;

    [SerializeField] private GameObject settingsMenu;

    [Inject] private IObjectResolver container; // Add this!

    [Button]
    private void InstantiateSettingsMenu()
    {
        container.Instantiate(settingsMenu);
    }
}
