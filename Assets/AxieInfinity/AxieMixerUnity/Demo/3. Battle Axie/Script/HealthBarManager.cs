using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    public int maxHealth
    {
        get
        {
            return this._maxHealth;
        }
        set
        {
            this._maxHealth = value;
            this.UpdateHealthBar();
        }
    }

    public int currentHealth {
        get
        {
            return this._currentHealth;
        }
        set
        {
            this._currentHealth = value;
            this.UpdateHealthBar();
        }
    }
    private                   int            _maxHealth = 10;
    private                   int            _currentHealth = 10;
    [SerializeField] private SpriteRenderer maxHealthBar;
    [SerializeField] private SpriteRenderer currentHealthBar;

    public float scaleRatio = 1;

    void UpdateHealthBar()
    {
        this.maxHealthBar.transform.localScale = new Vector3(this.maxHealth * this.scaleRatio, this.maxHealthBar.transform.localScale.y, this.maxHealthBar.transform.localScale.z);
        this.currentHealthBar.transform.localScale = new Vector3(this.currentHealth * this.scaleRatio, this.currentHealthBar.transform.localScale.y, this.currentHealthBar.transform.localScale.z);

    }
}
