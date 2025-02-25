// export R# package module type define for javascript/typescript language
//
//    imports "QSAR" from "mzkit";
//
// ref=mzkit.QSAR@mzkit, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

/**
 * QSAR, which stands for Quantitative Structure-Activity Relationship, is a computational
 *  methodology used in the fields of chemistry, biology, and pharmacology to analyze and 
 *  predict the relationship between the chemical structure of a molecule and its biological
 *  activity or properties.
 *  
 *  The basic principle of QSAR is that the biological activity of a molecule is related to its
 *  physicochemical properties, which can be described by various molecular descriptors. These
 *  descriptors can include factors such as molecular weight, hydrophobicity, electronic
 *  properties, and structural features like the presence of certain functional groups.
 *  
 *  Here's a more detailed breakdown of the QSAR process:
 *  
 *  1. **Data Collection**: A set of compounds with known biological activities is collected. 
 *     This dataset serves as the training set for the QSAR model.
 *  2. **Molecular Descriptors Calculation**: For each compound in the dataset, a series of 
 *     molecular descriptors are calculated. These descriptors quantitatively represent the 
 *     molecular structure and properties.
 *  3. **Model Development**: Statistical or machine learning methods are used to analyze the 
 *     relationship between the molecular descriptors and the biological activities. This results
 *     in a mathematical model that can predict the activity of new compounds based on their descriptors.
 *  4. **Validation**: The developed QSAR model is validated using a separate set of compounds
 *     (the test set) to ensure its predictive power and robustness.
 *  5. **Prediction**: Once validated, the QSAR model can be used to predict the biological 
 *     activities of new or untested compounds based on their molecular descriptors.
 *     
 *  QSAR has several applications, including:
 *  
 *  - **Drug Discovery**: Predicting the biological activity of potential drug candidates, thereby 
 *    reducing the number of compounds that need to be synthesized and tested in the lab.
 *  - **Environmental Chemistry**: Assessing the toxicity of chemicals and their environmental impact.
 *  - **Chemical Safety**: Evaluating the safety of chemicals used in consumer products.
 *  
 *  However, QSAR models also have limitations. They are based on the assumption that the molecular 
 *  descriptors adequately capture the relevant aspects of molecular structure that influence biological
 *  activity. Additionally, the quality of the predictions is highly dependent on the quality and
 *  diversity of the training data.
 *  
 *  In summary, QSAR is a powerful tool for predicting the biological activities of compounds based 
 *  on their chemical structures, with wide-ranging applications in various scientific disciplines.
 *  However, it requires careful validation and interpretation to ensure the reliability of its 
 *  predictions.
 * 
*/
declare namespace QSAR {
   /**
     * @param len default value Is ``null``.
     * @param env default value Is ``null``.
   */
   function fingerprint(len?: object, env?: object): object;
   /**
    * Morgan fingerprints, also known as circular fingerprints, are a type of molecular fingerprint 
    *  used in cheminformatics to represent the structure of chemical compounds. The algorithm steps 
    *  for generating Morgan fingerprints are as follows:
    *  
    *  1. **Initialization**:
    *   - Start with the initial set of atoms in the molecule.
    *   - Assign a unique identifier (e.g., integer) to each atom.
    *   
    *  2. **Atom Environment Encoding**:
    *   - For each atom, encode its immediate environment, which includes the atom type and the types of its directly connected neighbors.
    *   - This information can be represented as a string or a hash.
    *   
    *  3. **Iterative Expansion**:
    *   - Expand the environment encoding iteratively to include atoms further away from the starting atom.
    *   - In each iteration, update the encoding to include the types of atoms that are two, three, etc., bonds away from the starting atom.
    *   
    *  4. **Hashing**:
    *   - Convert the environment encoding into a fixed-size integer using a hashing function. This integer represents the fingerprint of the atom's environment.
    *   - Different atoms in the molecule will have different fingerprints based on their environments.
    *   
    *  5. **Circular Fingerprint Generation**:
    *   - For each atom, generate a series of fingerprints that represent its environment at different radii (number of bonds away).
    *   - The final fingerprint for an atom is a combination of these series of fingerprints.
    *   
    *  6. **Molecular Fingerprint**:
    *   - Combine the fingerprints of all atoms in the molecule to create the final molecular fingerprint.
    *   - This can be done by taking the bitwise OR of all atom fingerprints, resulting in a single fingerprint that represents the entire molecule.
    *   
    *  7. **Optional Folding**:
    *   - To reduce the size of the fingerprint, an optional folding step can be applied. This involves 
    *     dividing the fingerprint into chunks and performing a bitwise XOR operation within each chunk.
    *     
    *  8. **Result**:
    *   - The final result is a binary vector (or a list of integers) that represents the Morgan fingerprint 
    *     of the molecule. This fingerprint can be used for similarity searching, clustering, and other 
    *     cheminformatics tasks.
    *     
    *  Morgan fingerprints are particularly useful because they capture the circular nature of molecular
    *  environments, meaning that the path taken to reach an atom is not as important as the environment 
    *  around it. This makes them effective for comparing the similarity of molecules based on their 
    *  structural features.
    * 
    * 
     * @param itrs 
     * + default value Is ``3``.
   */
   function morgan_fingerprint(struct: object, itrs?: object): object;
}
