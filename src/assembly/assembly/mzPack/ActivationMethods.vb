
''' <summary>
''' the most common ion activation techniques employed in tandem mass spectrometry
''' </summary>
Public Enum ActivationMethods As Byte

    ''' <summary>
    ''' In collision-induced dissociation (CID), activation of the selected ions 
    ''' occurs by collision(s) with neutral gas molecules in a collision cell. 
    ''' This experiment can be done at high (keV) collision energies, using tandem 
    ''' sector and time-of-flight instruments, or at low (eV range) energies, in 
    ''' tandem quadrupole and ion trapping instruments.
    ''' </summary>
    CID
    HCD
    ''' <summary>
    ''' Collisional activation upon impact of precursor ions on solid surfaces, 
    ''' surface-induced dissociation (SID), is gaining importance as an alternative 
    ''' to gas targets and has been implemented in several different types of mass 
    ''' spectrometers.
    ''' </summary>
    SID
    ''' <summary>
    ''' unique fragmentation mechanisms of multiply-charged species can be studied 
    ''' by electron-capture dissociation (ECD). The ECD technique has been recognized 
    ''' as an efficient means to study non-covalent interactions and to gain 
    ''' sequence information in proteomics applications.
    ''' </summary>
    ECD
    ''' <summary>
    ''' Trapping instruments, such as quadrupole ion traps and Fourier transform ion 
    ''' cyclotron resonance instruments, are particularly useful for the photoactivation 
    ''' of ions, specifically for fragmentation of precursor ions by infrared 
    ''' multiphoton dissociation (IRMPD). IRMPD is a non-selective activation method 
    ''' and usually yields rich fragmentation spectra. 
    ''' </summary>
    IRMPD

End Enum
