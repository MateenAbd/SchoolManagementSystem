﻿namespace SMS.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IStudentRepository StudentRepository { get; }


    }
}
