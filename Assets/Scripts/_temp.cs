using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;



public class _temp 
{
    [SerializeField, BoxGroup("Setup")] private Splitter splitter;
    [SerializeField, BoxGroup("Setup")] private Image main;
    [SerializeField, BoxGroup("Setup")] private Image grid;
    [SerializeField] private List<LevelButton> buttons;

    public delegate void OnLevelButtonEvent(Sprite main, Sprite grid, SO_Data data);

    public static event OnLevelButtonEvent OnLevelButtonClick;

    protected  void OnEnable()
    {
        //base.OnEnable();
        buttons.ForEach(x => x.Init());
        OnLevelButtonClick += Run;
    }

    protected  void OnDisable()
    {
        //base.OnDisable();
        buttons.ForEach(x => x.Disable());
        OnLevelButtonClick -= Run;
    }

    private void Run(Sprite main, Sprite grid, SO_Data data)
    {
        splitter.Clear();
        this.main.sprite = main;
        this.grid.sprite = grid;
        splitter.data = data;
        splitter.Play();
        //Close();
    }



    [Serializable]
    private class LevelButton
    {
        [SerializeField] private Sprite main;
        [SerializeField] private Sprite grid;
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        [SerializeField] private SO_Data data;
        public Maps map;


        public void Init()
        {
            button.onClick.AddListener(OnClick);
            buttonImage.sprite = main;
        }

        public void Disable()
        {
            button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            OnLevelButtonClick?.Invoke(main, grid, data);
        }

    }
}
