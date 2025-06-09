// Example: How to resolve feat references in your GToonManager application

class FeatResolver {
    constructor() {
        this.originFeats = null;
        this.generalFeats = null;
    }

    // Load feat data files
    async loadFeatData() {
        const fs = require('fs').promises;
        
        // Load origin feats
        const originFeatsData = await fs.readFile('Data/feats/origin_feats.json', 'utf8');
        this.originFeats = JSON.parse(originFeatsData).feats;
        
        // Load general feats (optional, for non-origin feats)
        const generalFeatsData = await fs.readFile('Data/feats/feats.json', 'utf8');
        this.generalFeats = JSON.parse(generalFeatsData);
    }

    // Resolve a feat reference from a background
    resolveFeatReference(background) {
        if (!background.feat) {
            return null;
        }

        // Check if it's a reference or inline data
        if (background.feat.feat_reference && background.feat.source_file) {
            const featName = background.feat.feat_reference;
            const sourceFile = background.feat.source_file;
            
            if (sourceFile === 'origin_feats' && this.originFeats[featName]) {
                return {
                    name: featName,
                    ...this.originFeats[featName],
                    reference_source: 'origin_feats'
                };
            } else if (sourceFile === 'feats' && this.generalFeats[featName]) {
                return {
                    name: featName,
                    ...this.generalFeats[featName],
                    reference_source: 'feats'
                };
            } else {
                console.warn(`Feat reference not found: ${featName} in ${sourceFile}`);
                return null;
            }
        } else {
            // Handle legacy inline feat data
            const featNames = Object.keys(background.feat);
            if (featNames.length > 0) {
                const featName = featNames[0];
                return {
                    name: featName,
                    ...background.feat[featName],
                    reference_source: 'inline'
                };
            }
        }
        
        return null;
    }

    // Get complete background data with resolved feat
    async getCompleteBackground(backgroundPath) {
        const fs = require('fs').promises;
        
        // Load background data
        const backgroundData = await fs.readFile(backgroundPath, 'utf8');
        const background = JSON.parse(backgroundData);
        
        // Resolve the feat reference
        const resolvedFeat = this.resolveFeatReference(background);
        
        if (resolvedFeat) {
            background.feat = {
                [resolvedFeat.name]: {
                    ...resolvedFeat,
                    name: undefined, // Remove the redundant name field
                    reference_source: undefined // Remove metadata
                }
            };
        }
        
        return background;
    }
}

// Usage example:
async function example() {
    const resolver = new FeatResolver();
    await resolver.loadFeatData();
    
    // Get a background with resolved feat reference
    const urchinBackground = await resolver.getCompleteBackground('Data/backgrounds/2024/urchin (2024).json');
    
    console.log('Urchin Background Feat:');
    console.log(JSON.stringify(urchinBackground.feat, null, 2));
    
    // Or just resolve a feat reference directly
    const featData = resolver.resolveFeatReference(urchinBackground);
    console.log('Resolved Feat Data:');
    console.log(JSON.stringify(featData, null, 2));
}

module.exports = { FeatResolver }; 