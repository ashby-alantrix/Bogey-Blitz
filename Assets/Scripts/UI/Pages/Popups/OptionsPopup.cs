
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPopup : PopupBase
{
    [SerializeField] private Button musicBtn;
    [SerializeField] private Button soundBtn;
    [SerializeField] private Button backBtn;

    [SerializeField] private TextMeshProUGUI musicText;
    [SerializeField] private TextMeshProUGUI soundText;

    private PopupBackHandler popupBackHandler;
    private InGameSFXData inGameSFXData;
    private SoundManager soundManager;

    public void InitSFXData(InGameSFXData inGameSFXData, SoundManager soundManager)
    {
        this.soundManager = soundManager;
        this.inGameSFXData = inGameSFXData;

        SetMusicView();
        SetSoundView();
    }

    private void SetMusicView()
    {
        musicText.text = inGameSFXData.gameMusicToggle ? ArcticEscape_Constants.Toggle_On : ArcticEscape_Constants.Toggle_Off;
        soundManager.SetGameMusic(inGameSFXData.gameMusicToggle);
    }

    private void SetSoundView()
    {
        soundText.text = inGameSFXData.gameSoundToggle ? ArcticEscape_Constants.Toggle_On : ArcticEscape_Constants.Toggle_Off;
        soundManager.SetGameSound(inGameSFXData.gameSoundToggle);
    }

    private void OnClick_Music()
    {
        inGameSFXData.gameMusicToggle = !inGameSFXData.gameMusicToggle;
        SetMusicView();
    }

    private void OnClick_Sound()
    {
        inGameSFXData.gameSoundToggle = !inGameSFXData.gameSoundToggle;
        SetSoundView();
    }

    private void OnClick_Back()
    {
        popupBackHandler = popupBackHandler == null ? new PopupBackHandler(popupManager) : popupBackHandler;
        popupBackHandler.HandleBackClick(popupType);
    }

    private void OnEnable()
    {
        musicBtn.onClick.AddListener(OnClick_Music);
        soundBtn.onClick.AddListener(OnClick_Sound);
        backBtn.onClick.AddListener(OnClick_Back);
    }

    private void OnDisable()
    {
        musicBtn.onClick.RemoveAllListeners();
        soundBtn.onClick.RemoveAllListeners();
        backBtn.onClick.RemoveAllListeners();
    }
}
