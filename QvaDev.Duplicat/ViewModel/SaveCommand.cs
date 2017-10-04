﻿using QvaDev.Data;

namespace QvaDev.Duplicat.ViewModel
{
    public class SaveCommand : ICommand
    {
        public void Execute(DuplicatContext duplicatContext, object parameter = null)
        {
            duplicatContext.SaveChanges();
        }
    }
}
