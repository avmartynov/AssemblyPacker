using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Twidlle.Infrastructure
{
    /// <summary> Коллекция элементов с контролем редактирования. </summary>
    /// <remark>Все операции редактирования обеспечивают актуальность признака того, 
    /// что данные были изменены (свойство Changed).</remark>
    /// <typeparam name="TImmutable">Тип элемента. Он должен быть неизменяемый (Immutable).</typeparam>
    public class EditableCollection<TImmutable>
    {
        public ReadOnlyCollection<TImmutable> Items
        {
            get { return _items.AsReadOnly(); }
        }


        public void Add(TImmutable item)
        {
            _items.Add(item);
            _changed = true;
        }


        public void Update(TImmutable oldItem, TImmutable newItem)
        {
            _items.Remove(oldItem);
            _items.Add(newItem);
            _changed = true;
        }


        public void Remove(TImmutable item)
        {
            _items.Remove(item);
            _changed = true;
        }


        public void Clear()
        {
            _items.Clear();
            _changed = true;
        }


        public bool Changed { get { return _changed; } }


        public void Load(ICollectionStorage<TImmutable> storage)
        {
            _items.Clear();
            _items.AddRange(storage.Load());
            _changed = false;
        }


        public void Save(ICollectionStorage<TImmutable> storage)
        {
            storage.Save(_items);
            _changed = false;
        }


        private bool _changed;
        private readonly List<TImmutable> _items = new List<TImmutable>();
    }
}
