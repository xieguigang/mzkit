# Design of mzPack format

## Data Model

```schema
mzpack {

    meta {
        # the file name part of the source data which 
        # converts to current mzpack data file.
        source_name: string

        # current data is used for a specific data analysis 
        # applicatin
        application: enum(byte) {
            LCMS = 0
            # targeted and un-targeted data
            GCMS = 1
            LCMSMS = 2
            GCxGC = 3
            MSImaging = 4
        }

        # file format version
        version: integer[]

        # version of mzkit software that write this 
        # mzpack data file
        mzkit_version: integer[]

        # unix timestamp for record the time when mzkit 
        # software write this data file.
        time: double

        description: string
    }

    MS [{
        TIC: double
        BPC: double
        rt: double
        scan_id: string

        mz: double[]
        into: double[]

        # a collection of key-value pair to store the meta data
        # of current scan data. the meta value of this element
        # is string type
        meta: [{ key -> value }]

        # scan msn data
        products: [{

            precursor: double
            rt: double
            intensity: double
            
            #  1 for positive
            # -1 for negative
            #  0 for natural
            polarity: int16
            charge: int16

            activationMethod: enum(byte) {
                # Unknown activation type
                Unknown = -1
        
                # Collision-Induced Dissociation
                # 
                # In collision-induced dissociation (CID), activation of the selected ions 
                # occurs by collision(s) with neutral gas molecules in a collision cell. 
                # This experiment can be done at high (keV) collision energies, using tandem 
                # sector and time-of-flight instruments, or at low (eV range) energies, in 
                # tandem quadrupole and ion trapping instruments.
                CID = 0

                # Multi Photon Dissociation
                MPD = 1

                # Electron Capture Dissociation
                # 
                # unique fragmentation mechanisms of multiply-charged species can be studied 
                # by electron-capture dissociation (ECD). The ECD technique has been recognized 
                # as an efficient means to study non-covalent interactions and to gain 
                # sequence information in proteomics applications.
                ECD = 2

                # Pulsed Q Dissociation
                PQD = 3

                # Electron Transfer Dissociation
                # 
                # Electron transfer dissociation (ETD) involves transfer of an electron from a 
                # radical anion to the analyte cation and also produces c and z type ions, and 
                # has been implemented with wide variety of mass analyzers, most commonly ion 
                # traps.
                ETD = 4

                # High-energy Collision-induce Dissociation (psi-ms: beam-type collision-induced dissociation)
                HCD = 5

                # Any activation type
                AnyType = 6

                # Supplemental Activation
                SA = 7

                # Proton Transfer Reaction
                PTR = 8

                # Negative Electron Transfer Dissociation
                NETD = 9

                # Negative Proton Transfer Reaction
                NPTR = 10

                # Ultraviolet Photo Dissociation
                UVPD = 11

                # Mode A
                ModeA = 12

                # Mode B
                ModeB = 13

                # Mode C
                ModeC = 14

                # Mode D
                ModeD = 15

                # Mode E
                ModeE = 16

                # Mode F
                ModeF = 17

                # Mode G
                ModeG = 18

                # Mode H
                ModeH = 19

                # Mode I
                ModeI = 20

                # Mode J
                ModeJ = 21

                # Mode K
                ModeK = 22

                # Mode L
                ModeL = 23

                # Mode M
                ModeM = 24

                # Mode N
                ModeN = 25

                # Mode O
                ModeO = 26

                # Mode P
                ModeP = 27

                # Mode Q
                ModeQ = 28

                # Mode R
                ModeR = 29

                # Mode S
                ModeS = 30

                # Mode T
                ModeT = 31

                # Mode U
                ModeU = 32

                # Mode V
                ModeV = 33

                # Mode W
                ModeW = 34

                # Mode X
                ModeX = 35

                # Mode Y
                ModeY = 36

                # Mode Z
                ModeZ = 37

                # Last Activation
                LastActivation = 38

                # Collisional activation upon impact of precursor ions on solid surfaces, 
                # surface-induced dissociation (SID), is gaining importance as an alternative 
                # to gas targets and has been implemented in several different types of mass 
                # spectrometers.
                SID

                # Trapping instruments, such as quadrupole ion traps and Fourier transform ion 
                # cyclotron resonance instruments, are particularly useful for the photoactivation 
                # of ions, specifically for fragmentation of precursor ions by infrared 
                # multiphoton dissociation (IRMPD). IRMPD is a non-selective activation method 
                # and usually yields rich fragmentation spectra. 
                IRMPD
            }

            collisionEnergy: single

            # 0 for false(profile)
            # 1 for true(centroid)
            centroided: byte

            # tree graph of the msn data
            # the msn products data struct keeps the same
            # as current object schema
            msn: self[]
        }] 
    }]

    # the content data in this section has been 
    # interpolate via bspline or reduced resolution
    # if the data size is too large
    chromatogram {
        scan_time: double[]
        TIC: double[]
        BPC: double[]
    }

}
```