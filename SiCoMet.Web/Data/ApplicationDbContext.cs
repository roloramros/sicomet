using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SiCoMet.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<RegistroAcceso> RegistrosAcceso => Set<RegistroAcceso>();
    public DbSet<RegistroAuditoria> RegistrosAuditoria => Set<RegistroAuditoria>();

    public DbSet<Area> Areas => Set<Area>();
    public DbSet<TipoInstrumento> TiposInstrumento => Set<TipoInstrumento>();
    public DbSet<Instrumento> Instrumentos => Set<Instrumento>();
    public DbSet<Calibracion> Calibraciones => Set<Calibracion>();
}
