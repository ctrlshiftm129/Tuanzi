
using System;

public class EconomicManager : Manager
{
    private ManagerLocator m_locator;
    private UIManager m_uiManager;
    private EnemyManager m_enemyManager;
    
    private int m_money;
    
    private void Awake()
    {
        m_locator = ManagerLocator.Instance;
        Register2Locator();
    }

    private void Start()
    {
        m_uiManager = m_locator.Get<UIManager>();
        m_enemyManager = m_locator.Get<EnemyManager>();
        m_enemyManager.onEnemyDead += OnEnemyDead;
        m_uiManager.ShowMoney(m_money);
    }

    private void OnDisable()
    {
        m_enemyManager.onEnemyDead -= OnEnemyDead;
    }

    #region Kill Enemy

    private void OnEnemyDead(Enemy enemy)
    {
        ++m_money;
        m_uiManager.ShowMoney(m_money);
    }

    #endregion
}
