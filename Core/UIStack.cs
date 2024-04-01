using System;
using System.Collections.Generic;
using System.Linq;

namespace UIFramework
{
    public class UIStack
    {
        private readonly Stack<IUIPanel> m_stack = new Stack<IUIPanel>();

        public int Count => m_stack.Count;

        public void Push(IUIPanel target)
        {
            if (m_stack.TryPeek(out var panel))
            {
                panel.Pause();
            }

            m_stack.Push(target);

            target.SetBelong(this);
            target.Open();
        }

        public void Pop(IUIPanel target)
        {
            if (!m_stack.Contains(target))
            {
                return;
            }

            IUIPanel panel = null;
            while ((panel != target) && m_stack.TryPop(out panel))
            {
                panel.SetBelong(null);
                panel.Close();
            }

            if (m_stack.TryPeek(out panel))
            {
                panel.Resume();
            }
        }

        public void Pop(int count)
        {
            count = Math.Min(count, m_stack.Count);
            if (count <= 0)
            {
                return;
            }

            IUIPanel panel;
            while (--count >= 0)
            {
                panel = m_stack.Pop();
                panel.SetBelong(null);
                panel.Close();
            }

            if (m_stack.TryPeek(out panel))
            {
                panel.Resume();
            }
        }

        public void Pop<T>() where T : IUIPanel
        {
            var panel = m_stack.OfType<T>().FirstOrDefault();
            if (panel != null)
            {
                Pop(panel);
            }
        }

        public void Clear()
        {
            foreach(var panel in m_stack)
            {
                panel.SetBelong(null);
            }
            m_stack.Clear();
        }
    }
}
