﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FaceRecognitionDatabaseEntities : DbContext
    {
        public FaceRecognitionDatabaseEntities()
            : base("name=FaceRecognitionDatabaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AverageVector> AverageVectors { get; set; }
        public virtual DbSet<EigenFace> EigenFaces { get; set; }
        public virtual DbSet<Wage> Wages { get; set; }
    }
}