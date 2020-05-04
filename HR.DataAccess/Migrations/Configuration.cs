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
                c => c.Name,
                new Candidate { Name = "Vadim0", LastName = "Dmitriev0", Email = "vadim0@yandex.ru", Patronymic = "Andreevich", PhoneNumber = "+79998048620" },
                new Candidate { Name = "Vadim1", LastName = "Dmitriev1", Email = "vadim1@yandex.ru", Patronymic = "Andreevich1", PhoneNumber = "+79998048621" },
                new Candidate { Name = "Vadim2", LastName = "Dmitriev2", Email = "vadim2@yandex.ru", Patronymic = "Andreevich2", PhoneNumber = "+79998048622" },
                new Candidate { Name = "Vadim3", LastName = "Dmitriev3", Email = "vadim3@yandex.ru", Patronymic = "Andreevich3", PhoneNumber = "+79998048623" },
                new Candidate { Name = "Vadim4", LastName = "Dmitriev4", Email = "vadim4@yandex.ru", Patronymic = "Andreevich4", PhoneNumber = "+79998048624" },
                new Candidate { Name = "Vadim5", LastName = "Dmitriev5", Email = "vadim5@yandex.ru", Patronymic = "Andreevich5", PhoneNumber = "+79998048625" }
            );

            context.Positions.AddOrUpdate(
                p => p.Name,
                new Position { Name = "Developer" },
                new Position { Name = "Analyst" },
                new Position { Name = "Engineer" },
                new Position { Name = "Architect" },
                new Position { Name = "Product owner" },
                new Position { Name = "QA" }
            );

            context.Companies.AddOrUpdate(
                com => com.Name,
                new Company { Name = "Luxoft" },
                new Company { Name = "Lanit" },
                new Company { Name = "Sberbank" },
                new Company { Name = "Alfa-Bank" },
                new Company { Name = "Tinkoff" },
                new Company { Name = "VTB" }
            );
        }
    }
}
