

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -debug -d:DEBUG
ASSEMBLY = ./bin/Sugar.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin


endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4
ASSEMBLY = ./bin/Sugar.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = ./bin


endif


LINUX_PKGCONFIG = \
	$(SUGAR_PC)  


	
all: $(ASSEMBLY) $(LINUX_PKGCONFIG) 

FILES = \
	AssemblyInfo.cs \
	Window.cs \
	DBusActivity.cs \
	Activity.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	Style.cs \
	Toolbox.cs \
	DBusDatastore.cs \
	sugar.pc.in 

REFERENCES =  \
	$(GTK_SHARP_20_LIBS) \
	System \
	Mono.Posix \
	$(NDESK_DBUS_10_LIBS) \
	$(NDESK_DBUS_GLIB_10_LIBS)

DLL_REFERENCES = 

SUGAR_PC = $(BUILD_DIR)/sugar.pc

$(SUGAR_PC): sugar.pc
	mkdir -p $(BUILD_DIR)
	cp '$<' '$@'



$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(build_resx_resources) : %.resources: %.resx
	resgen2 '$<' '$@'

$(ASSEMBLY) $(ASSEMBLY_MDB): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list)
	mkdir -p $(dir $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)

include $(top_srcdir)/Makefile.include
