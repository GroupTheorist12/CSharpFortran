# Interfacing C# with Fortran

### Introduction

Fortran is not a dead language.  Much scientific and financial code is written in Fortran. An example would be the BLAS library (Basic Linear Algebra Subprograms). To be able to call this huge base of code from a modern language such as C# is a very useful tool in the scientific programmer's tool belt. 

The code in this git gives examples of calling Fortran from C# using the C# P/Invoke method.

The assumption is that you have *gfortran*, *make* and the *.NET 5.0 SDK* installed either on Linux or on Windows in the WSL Linux subsystem. If not, directions for Ubuntu are given below.

#### Install gfortran

```
sudo apt-get install gfortran
```

#### Install make

```
sudo apt-get install build-essential
```

#### Install .NET 5.0 SDK

[Install the .NET SDK]: https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu



### Modern Fortran

From Fortran Wiki:

`iso_c_binding` is a standard intrinsic module which defines named constants, types, and procedures for [C interoperability](http://fortranwiki.org/fortran/show/C+interoperability).

[iso_c_binding]: http://fortranwiki.org/fortran/show/iso_c_binding

The code in this git exploits this interoperability to call Fortran routines from C# using P/Invoke (Platform Invocation Services). Below is a Fortran subroutine we wish to call:

#### Example Fortran routine that can be called from C#. 

```fortran
subroutine myfortsub ( input_string ) bind ( C, name="myfortsub" )

   use iso_c_binding, only: C_CHAR, c_null_char
   implicit none

   character (kind=c_char, len=1), dimension (10), intent (in) :: input_string
   character (len=10) :: regular_string
   integer :: i

   regular_string = " "
   loop_string: do i=1, 10
      if ( input_string (i) == c_null_char ) then
         exit loop_string
      else
         regular_string (i:i) = input_string (i)
      end if
   end do loop_string

   write (*, *) ">", trim (regular_string), "<", len_trim (regular_string)

   return

end subroutine myfortsub

```

#### Example C# P/Invoke declaration to call subroutine above

```c#
 [DllImport("myfortsub.so", CallingConvention = CallingConvention.Cdecl)]
 static extern void myfortsub(string str);
```

### Example C# console project

Let's walk through the steps to create a C# .NET 5.0 project to call the above Fortran subroutine.

1. #### Create console app using *dotnet*  cli.

   Open a terminal window and enter the following (I am doing it from my HOME directory):

   ```bash
   $ dotnet new console -o CallFortranFromCSharp
   ```

   Change directory to *CallFortranFromCSharp*

   ```bash
   $ cd CallFortranFromCSharp
   ```

2. #### Edit **Program.cs** 

   Edit the *Program.cs* file to look as below:

   ```c#
   using System;
   using System.Runtime.InteropServices;
   
   namespace CallFortranFromCSharp
   {
       class Program
       {
           [DllImport("myfortsub.so", CallingConvention = CallingConvention.Cdecl)]
            static extern void myfortsub(string str);
   
           static void Main(string[] args)
           {
             myfortsub("Howdy");
           }
       }
   }
   ```

3. #### Create file myfortsub.f95 and build shared library.

   Cut and paste the Fortran subroutine above in your favorite editor and save it in the *CallFortranFromCSharp* directory as *myfortsub.f95*.  

   In the terminal window run the following command to create a shared library.

   ```bash
   $ gfortran -shared -O2 myfortsub.f95 -o myfortsub.so -fPIC
   ```

4. #### Build C# console project and run it.

   Run the following from the terminal window in the *CallFortranFromCSharp* directory.

   ```bash
   $ dotnet build
   ```

   Once it completes, copy the Fortran shared library to */Debug/net5.0*.

   ```bash
   $ cp myfortsub.so bin/Debug/net5.0
   ```

   Run  the following from the terminal window to run the app.

   ```bash
   $ dotnet run
   ```

   You should see the following output:

   ```bash
   $ dotnet run
    >Howdy<           5
   ```

   

### Using code in this git

The code in this git gives examples of passing strings, arrays and structs to Fortran from C#.

Modern Fortran (as of 2003) has the concept of a module to organize your subroutines and structures. 

From the Fortran Wiki:

## Module

**Modules** are used for object oriented programming.

### General form

The general form is

```fortran
module <name>
  <use statements>  
  <declarations>
contains
  <subroutines and functions>
end module <name>
```

[Fortran Module]: http://fortranwiki.org/fortran/show/Module

#### Module csharp_module.f95

In the git source *csharp_module* directory we have the source file csharp_module.f95. It has three example subroutines:

1. **PassString** - Allows C# to pass a C# string and the string length to Fortran.

   ```fortran
   subroutine PassString ( input_string, n) bind ( C, name="PassString" )
   
           use iso_c_binding, only: C_CHAR, c_null_char
           implicit none
        
           character (kind=c_char, len=1), dimension (n), intent (in) :: input_string
           integer(c_int), intent(in) :: n
   
           character (len=n) :: regular_string
           integer :: i
        
           regular_string = " "
           loop_string: do i=1, n
              if ( input_string (i) == c_null_char ) then
                 exit loop_string
              else
                 regular_string (i:i) = input_string (i)
              end if
           end do loop_string
        
           write (*, *) ">", trim (regular_string), "<", len_trim (regular_string)
        
           return
        
        end subroutine PassString
   ```

   Fortran uses character arrays so the c string needs to be converted to a Fortran character array.

2. **DegCtoF** - Allows C# to pass arrays of floats. One array contains the Centigrade values to be converted and the subroutine fills the other array with Fahrenheit values.

   ```fortran
    subroutine DegCtoF(degC, degF, n)&
           bind(c, name = "DegCtoF")
       
           integer(c_int), intent(in) :: n
           real(c_double), intent(in), dimension(n) :: degC
           real(c_double), intent(out), dimension(n) :: degF
           integer :: i
       
           do i = 1, n
               degF(i) = ( degC(i) * 1.8 ) + 32
           end do
       
   end subroutine DegCtoF
   ```

   Notice the use of *intent* keyword to denote if value is an inbound, outbound or both value.

   From the *Fortran Wiki*:

   The `intent(in)` attribute of argument `n` means that `n` cannot be changed inside the subroutine.

   The `intent(out)` attribute of argument `degF` means that `degF` can be changed inside the subroutine.

   The `intent(inout)` attribute means it can be changed outside and inside the subroutine.

   

3. **type, bind(c) :: point3d** - Fortran *type* is similar to *C# struct*. We denote the *point3d* type as having c bindings and it's members are all of c types such as *real(c_float):: x* .

   ```fortran
   type, bind(c) :: point3d
           real(c_float):: x
           real(c_float) :: y
           real(c_float) :: z
       end type point3d
   ```

    

4. **fflip** - Allows C# to pass *struct* pointers (using ref) to Fortran.

   ```fortran
    subroutine fflip(p) bind(c,name='fflip') ! bind with the fflip name 
            
           !declare our struct that will be passed. will pass in and values
           !and pass back values flipped
           type(point3d), intent(inout) :: p 
   
           real :: T
   
     
           T = p%x
           p%x = p%y
           p%y = T
           p%z = -2.*p%z
   
   
       end subroutine fflip
   
   ```

   Notice the use of `intent(inout)` on the *point3d* subroutine parameter *p*. This allows the parameter to be modified by the subroutine.

### Using the code in the git

The code in the git can be cloned and ran if *gfortran*, *make* and *.Net 5.0* are installed either in Windows WSL or a Linux instance.

1. **Clone code**

   ```bash
   $ git clone https://github.com/GroupTheorist12/CSharpFortran.git
   ```

2. **Chmod runit.sh bash script**

   ```bash
   cd CSharpFortran
   $ chmod 777 runit.sh
   ```

3. **Run runit.sh bash script**

   ```bash
   $ ./runit.sh
   ```

   produces the following output:

   ```bash
   make: Entering directory '/home/brigg/CSharpFortran/csharp_module'
   gfortran -shared -O2 csharp_module.f95 -o csharp_module.so -fPIC
   make: Leaving directory '/home/brigg/CSharpFortran/csharp_module'
    >Howdy Doody Man<          15
   0 : 32.00 [C] = 89.60 [F]
   
   1 : 64.00 [C] = 147.20 [F]
   
   values before fortran call: x = 3.00, y = 7.00, z = 9.00
   values after fortran call: x = 7.00, y = 3.00, z = -18.00
   ```

   