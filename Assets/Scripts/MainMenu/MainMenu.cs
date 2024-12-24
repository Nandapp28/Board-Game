using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [System.Serializable]
    public class MenuTransformAnimation
    {
        [Header("Start")]
        public Vector3 StartPosition;
        public float StartRotation; // Menggunakan float untuk sudut rotasi
        public Vector3 StartScale;

        [Header("End")]
        public Vector3 EndPosition;
        public float EndRotation; // Menggunakan float untuk sudut rotasi
        public Vector3 EndScale;
    }

    [System.Serializable]
    public class ModeMenu
    {
        public GameObject modeParentContainer;
        public Button singlePlayer;
        public MenuTransformAnimation transformAnimation; // Perbaikan di sini
        public float modeMenuContainerDuration;
    }

    public class SelectPlayer
    {
        public GameObject selectParentContainer;
        public GameObject SelectParent;
        public Button selectButton;
    }

    public ModeMenu modeMenu;
    public SelectPlayer selectPlayer;
    public Button backButton;

    private void Start() {
        // Menambahkan listener untuk tombol
        modeMenu.singlePlayer.onClick.AddListener(SinglePlayerButton);
        ModeMenuAnimation(); // Memanggil animasi saat mulai
    }

    #region Mode Menu

    public void ModeMenuAnimation()
    {
        modeMenu.modeParentContainer.SetActive(true);
        StartTransform();
        AnimationTransform(modeMenu.modeParentContainer,modeMenu.transformAnimation.EndPosition,modeMenu.transformAnimation.EndRotation,modeMenu.transformAnimation.EndScale,modeMenu.modeMenuContainerDuration);
    }

    public void StartTransform()
    {
        modeMenu.modeParentContainer.transform.localPosition = modeMenu.transformAnimation.StartPosition;
        modeMenu.modeParentContainer.transform.localEulerAngles = new Vector3(0, 0, modeMenu.transformAnimation.StartRotation);
        modeMenu.modeParentContainer.transform.localScale = modeMenu.transformAnimation.StartScale;
    }

    public void SinglePlayerButton()
    {
        AnimationTransform(modeMenu.modeParentContainer,modeMenu.transformAnimation.StartPosition,modeMenu.transformAnimation.StartRotation,modeMenu.transformAnimation.StartScale,modeMenu.modeMenuContainerDuration);
    }

    #endregion

    #region Select Player

    #endregion

    public void AnimationTransform(GameObject obj,Vector3 position , float rotation, Vector3 scale , float duration)
    {
        // Mengatur animasi untuk posisi, rotasi, dan skala
        obj.transform.DOLocalMove(position, duration);

        obj.transform.DOLocalRotate(new Vector3(0, 0, rotation), duration);

        obj.transform.DOScale(scale, duration);
    }

    public void BackButton()
    {

    }
}