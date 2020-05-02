namespace HR.DataAccess.Migrations
{
    using HR.Model;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<HR.DataAccess.HrDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(HR.DataAccess.HrDbContext context)
        {
            context.Candidates.AddOrUpdate(
                c=>c.Name,
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim", LastName = "Dmitriev", Email = "vadim-cavs@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048624" }
            );
        }
    }
}
