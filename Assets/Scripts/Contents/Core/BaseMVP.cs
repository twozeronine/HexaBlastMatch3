using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMVP
{
    public interface IPresenter
    {
        public void LoadData();
        public void CreateModel();
    }
    
    public interface IView 
    {
        public void SetLayout<T>(T data);
        public GameObject GetInstance();
    }
    
    public interface IModel
    {
        public void Init();
    }
}
