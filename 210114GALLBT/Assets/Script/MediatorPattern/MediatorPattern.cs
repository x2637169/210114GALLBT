using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DesignPattern_Mediator
{
    public abstract class Colleague
    {
        protected Mediator m_Mediator = null;

        public Colleague(Mediator theMediator)
        {
            m_Mediator = theMediator;
        }
        public abstract void Request(string Message);
    }
    public abstract class Mediator
    {
        public abstract void SendMessage(Colleague theColleague, string Message);
    }
    public class ConcreteColleague1 : Colleague
    {
        public ConcreteColleague1(Mediator theMediator) :
            base(theMediator)
        { }
        public void Acton()
        {
            m_Mediator.SendMessage(this, "Colleague1發出通知");
        }
        public override void Request(string Message)
        {
            //Debug.Log("ConcreteColleague1.Request:" + Message);
        }
    }
    public class ConcreteColleague2 : Colleague
    {
        public ConcreteColleague2(Mediator theMediator) :
            base(theMediator)
        { }
        public void Acton()
        {
            m_Mediator.SendMessage(this, "Colleague2發出通知");
        }
        public override void Request(string Message)
        {
            //Debug.Log("ConcreteColleague2.Request:" + Message);
        }
    }

    public class ConcreteMediator : Mediator
    {

        ConcreteColleague1 m_Colleague1 = null;
        ConcreteColleague2 m_Colleague2 = null;
        public void SetColleague1(ConcreteColleague1 theColleague)
        {
            m_Colleague1 = theColleague;
        }
        public void SetColleague2(ConcreteColleague2 theColleague)
        {
            m_Colleague2 = theColleague;
        }
        public override void SendMessage(Colleague theColleague, string Message)
        {
            if (m_Colleague1 == theColleague) m_Colleague2.Request(Message);
            if (m_Colleague2 == theColleague) m_Colleague1.Request(Message);
        }
    }
}

