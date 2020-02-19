using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public GameObject MainMenuCanvas;
    public GameObject TutorialCanvas;
    public GameObject TutorialCanvas2;
    public GameObject CreditsCanvas;

    // Use this for initialization
    void Start () {
        OpenMainMenu();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OpenMainMenuByButton() {
        SoundEffects.sf.playButton();
        OpenMainMenu();
    }

    public void OpenMainMenu() {
        TutorialCanvas.SetActive(false);
        TutorialCanvas2.SetActive(false);
        CreditsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }

    public void InitiateGame() {
        SoundEffects.sf.playButton();
        Manager.aiActive = false;
        SceneManager.LoadScene("Scene");
    }

    public void InitiateGameAI()
    {
        SoundEffects.sf.playButton();
        Manager.aiActive = true;
        SceneManager.LoadScene("Scene");
    }

    public void TutorialUI() {
        SoundEffects.sf.playButton();
        MainMenuCanvas.SetActive(false);
        TutorialCanvas2.SetActive(false);
        TutorialCanvas.SetActive(true);
    }

    public void TutorialUI2()
    {
        SoundEffects.sf.playButton();
        TutorialCanvas.SetActive(false);
        TutorialCanvas2.SetActive(true);
    }

    public void CreditsUI() {
        SoundEffects.sf.playButton();
        MainMenuCanvas.SetActive(false);
        CreditsCanvas.SetActive(true);
    }

    public void ExitGame() {
        Application.Quit();
    }
}
