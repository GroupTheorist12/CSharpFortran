module csharp_module
    use, intrinsic :: iso_c_binding !allow c type bindings and function names
    implicit none !types must be declared

    !type with c bindings for passing in from c# pinvoke calls
    type, bind(c) :: point3d
        real(c_float):: x
        real(c_float) :: y
        real(c_float) :: z
    end type point3d

    contains !what subs we have

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

    !
    ! Convert temperature degrees Celsius Fahrenheit
    ! degC, input array of celsius values
    ! degF output array of Fahrenheit
    ! n number of values in arrays
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
     
end module csharp_module     